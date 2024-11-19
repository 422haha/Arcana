package com.arcane.arcana.subscribe.service;

import com.arcane.arcana.common.entity.Subscribe;
import com.arcane.arcana.subscribe.repository.SubscribeRepository;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.mail.javamail.MimeMessagePreparator;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;
import org.thymeleaf.context.Context;
import org.thymeleaf.spring6.SpringTemplateEngine;

@Service
public class SubscribeServiceImpl implements SubscribeService {

    private final SubscribeRepository subscribeRepository;
    private final JavaMailSender mailSender;
    private final SpringTemplateEngine templateEngine;

    @Value("${spring.mail.username}")
    private String senderEmail;

    @Value("${app.domain}")
    private String appDomain;

    @Autowired
    public SubscribeServiceImpl(SubscribeRepository subscribeRepository, JavaMailSender mailSender,
        SpringTemplateEngine templateEngine) {
        this.subscribeRepository = subscribeRepository;
        this.mailSender = mailSender;
        this.templateEngine = templateEngine;
    }

    @Override
    @Transactional
    public void subscribe(String email) {
        if (subscribeRepository.existsBySubscribe(email)) {
            throw new CustomException("이미 구독된 이메일입니다.", HttpStatus.BAD_REQUEST);
        }
        Subscribe subscribe = new Subscribe();
        subscribe.setSubscribe(email);
        subscribeRepository.save(subscribe);

        // 구독 완료 이메일 전송
        sendSubscriptionConfirmationEmail(email);
    }

    private void sendSubscriptionConfirmationEmail(String recipientEmail) {
        String subject = "Arcana 구독 완료";

        // Thymeleaf 컨텍스트 설정
        Context context = new Context();
        context.setVariable("email", recipientEmail);

        // 템플릿을 HTML 문자열로 변환
        String htmlContent = templateEngine.process("subscribe-confirmation", context);

        MimeMessagePreparator messagePreparator = mimeMessage -> {
            MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true, "UTF-8");
            helper.setFrom(senderEmail, "Arcana Team");
            helper.setTo(recipientEmail);
            helper.setSubject(subject);
            helper.setText(htmlContent, true); // HTML 형식 설정
        };

        mailSender.send(messagePreparator);
    }
}

