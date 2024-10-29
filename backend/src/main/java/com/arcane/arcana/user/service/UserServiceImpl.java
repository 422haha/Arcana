package com.arcane.arcana.user.service;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.dto.RegisterDto;
import com.arcane.arcana.user.dto.UpdateDto;
import com.arcane.arcana.user.dto.LoginDto;
import com.arcane.arcana.user.dto.LoginResponseDto;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.common.util.JwtUtil;
import com.arcane.arcana.common.service.RedisService;
import com.arcane.arcana.common.exception.CustomException;
import jakarta.mail.internet.MimeMessage;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpStatus;
import org.springframework.mail.javamail.JavaMailSender;
import org.springframework.mail.javamail.MimeMessageHelper;
import org.springframework.mail.javamail.MimeMessagePreparator;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.Optional;
import java.util.Random;

import com.fasterxml.jackson.databind.ObjectMapper;

/**
 * 사용자 서비스 클래스 구현
 */
@Service
public class UserServiceImpl implements UserService {

    private final UserRepository userRepository;
    private final PasswordEncoder passwordEncoder;
    private final JwtUtil jwtUtil;
    private final RedisService redisService;
    private final JavaMailSender mailSender;
    private final ObjectMapper objectMapper;
    private final AuthenticationManager authenticationManager;

    @Value("${spring.mail.username}")
    private String senderEmail;

    @Value("${app.domain}")
    private String appDomain;

    public UserServiceImpl(UserRepository userRepository, PasswordEncoder passwordEncoder,
        JwtUtil jwtUtil, RedisService redisService, JavaMailSender mailSender,
        AuthenticationManager authenticationManager) {
        this.userRepository = userRepository;
        this.passwordEncoder = passwordEncoder;
        this.jwtUtil = jwtUtil;
        this.redisService = redisService;
        this.mailSender = mailSender;
        this.authenticationManager = authenticationManager;
        this.objectMapper = new ObjectMapper();
    }

    @Override
    @Transactional
    public void registerUser(RegisterDto registerDto) {
        if (userRepository.existsByEmail(registerDto.getEmail())) {
            throw new CustomException("이미 존재하는 이메일입니다.", HttpStatus.BAD_REQUEST);
        }

        if (userRepository.existsByNickname(registerDto.getNickname())) {
            throw new CustomException("이미 존재하는 닉네임입니다.", HttpStatus.BAD_REQUEST);
        }

        User user = new User();
        user.setEmail(registerDto.getEmail());
        user.setNickname(registerDto.getNickname());
        user.encodePassword(registerDto.getPassword(), passwordEncoder);
        userRepository.save(user);

        String emailVerificationToken = jwtUtil.generateEmailVerificationToken(user.getEmail());
        String emailVerificationCode = generateVerificationCode();

        redisService.setStringValue("email_verification:" + user.getEmail(), emailVerificationToken,
            60); // 60분 유효
        redisService.setStringValue("email_code:" + user.getEmail(), emailVerificationCode,
            60); // 60분 유효

        sendVerificationEmail(user.getEmail(), emailVerificationToken, emailVerificationCode);
    }

    private void sendVerificationEmail(String recipientEmail, String token, String code) {
        String verificationUrl =
            appDomain + "/user/verify-email?email=" + recipientEmail + "&token=" + token;
        String subject = "이메일 인증";
        String content = "<p>안녕하세요!</p>"
            + "<p>Arcana 서비스에 가입해 주셔서 감사합니다.</p>"
            + "<p>아래 링크를 클릭하여 이메일 인증을 완료하거나, 인증번호를 입력해 주세요:</p>"
            + "<a href=\"" + verificationUrl + "\">이메일 인증하기</a>"
            + "<p>인증번호: <b>" + code + "</b></p>"
            + "<p>감사합니다.<br>Arcana 팀</p>";

        MimeMessagePreparator messagePreparator = mimeMessage -> {
            MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true, "UTF-8");
            helper.setFrom(senderEmail, "Arcana Team");
            helper.setTo(recipientEmail);
            helper.setSubject(subject);
            helper.setText(content, true); // HTML 텍스트 설정
        };

        mailSender.send(messagePreparator);
    }

    private String generateVerificationCode() {
        Random random = new Random();
        int code = 100000 + random.nextInt(900000); // 6자리 인증번호 생성
        return String.valueOf(code);
    }

    @Override
    public void verifyEmail(String email, String tokenOrCode) {
        if (tokenOrCode.length() == 6) {
            verifyEmailWithCode(email, tokenOrCode);
        } else {
            verifyEmailWithToken(email, tokenOrCode);
        }
    }

    private void verifyEmailWithToken(String email, String token) {
        String redisToken = redisService.getStringValue("email_verification:" + email);
        if (redisToken != null && redisToken.equals(token)) {
            Optional<User> userOpt = userRepository.findByEmail(email);
            if (userOpt.isPresent()) {
                User user = userOpt.get();
                user.setEmailVerified(true);
                userRepository.save(user);
                redisService.deleteValue("email_verification:" + email);
                redisService.deleteValue("email_code:" + email);
            } else {
                throw new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND);
            }
        } else {
            throw new CustomException("유효하지 않은 토큰입니다.", HttpStatus.BAD_REQUEST);
        }
    }

    private void verifyEmailWithCode(String email, String code) {
        String redisCode = redisService.getStringValue("email_code:" + email);
        if (redisCode != null && redisCode.equals(code)) {
            Optional<User> userOpt = userRepository.findByEmail(email);
            if (userOpt.isPresent()) {
                User user = userOpt.get();
                user.setEmailVerified(true);
                userRepository.save(user);
                redisService.deleteValue("email_code:" + email);
                redisService.deleteValue("email_verification:" + email);
            } else {
                throw new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND);
            }
        } else {
            throw new CustomException("유효하지 않은 인증번호입니다.", HttpStatus.BAD_REQUEST);
        }
    }

    @Override
    @Transactional
    public void updateUser(Long userId, UpdateDto updateDto) {
        User user = userRepository.findById(userId)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        // 닉네임 변경 시
        if (updateDto.getNickname() != null && !updateDto.getNickname().isEmpty() &&
            !updateDto.getNickname().equals(user.getNickname())) {
            if (userRepository.existsByNickname(updateDto.getNickname())) {
                throw new CustomException("이미 존재하는 닉네임입니다.", HttpStatus.BAD_REQUEST);
            }
            user.setNickname(updateDto.getNickname());
        }

        // 비밀번호 변경 시
        if (updateDto.getPassword() != null && !updateDto.getPassword().isEmpty()) {
            if (updateDto.getOldPassword() == null || !passwordEncoder.matches(
                updateDto.getOldPassword(), user.getPassword())) {
                throw new CustomException("기존 비밀번호가 일치하지 않습니다.", HttpStatus.BAD_REQUEST);
            }
            user.encodePassword(updateDto.getPassword(), passwordEncoder);
        }

        userRepository.save(user);
    }

    @Override
    public void logout(String email, String accessToken) {
        redisService.deleteValue("refresh_token:" + email);
        long expirationMillis =
            jwtUtil.getExpirationFromToken(accessToken).getTime() - System.currentTimeMillis();
        if (expirationMillis > 0) {
            redisService.blacklistToken(accessToken, expirationMillis);
        }
    }

    @Override
    public Long getUserIdByEmail(String email) {
        return userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND))
            .getId();
    }

    @Override
    public void updateRefreshToken(String email, String newRefreshToken) {
        redisService.setStringValue("refresh_token:" + email, newRefreshToken,
            jwtUtil.getRefreshTokenExpirationMinutes());
    }

    @Override
    public String getStoredRefreshToken(String email) {
        return redisService.getStringValue("refresh_token:" + email);
    }

    @Override
    @Transactional
    public void saveLanguage(String email, String language) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));
        user.setLanguage(language);
        userRepository.save(user);
    }

    @Override
    public boolean isNicknameAvailable(String nickname) {
        return !userRepository.existsByNickname(nickname);
    }

    @Override
    public void sendPasswordResetEmail(String email) {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("이메일을 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        String resetToken = jwtUtil.generatePasswordResetToken(email);
        redisService.setStringValue("password_reset:" + email, resetToken, 60); // 60분 유효

        sendPasswordResetEmail(email, resetToken);
    }

    private void sendPasswordResetEmail(String recipientEmail, String token) {
        String resetUrl =
            appDomain + "/user/reset-password?email=" + recipientEmail + "&token=" + token;
        String subject = "비밀번호 재설정";
        String content = "<p>안녕하세요!</p>"
            + "<p>Arcana 서비스의 비밀번호 재설정을 요청하셨습니다.</p>"
            + "<p>아래 링크를 클릭하여 비밀번호를 재설정해 주세요:</p>"
            + "<a href=\"" + resetUrl + "\">비밀번호 재설정하기</a>"
            + "<p>감사합니다.<br>Arcana 팀</p>";

        MimeMessagePreparator messagePreparator = mimeMessage -> {
            MimeMessageHelper helper = new MimeMessageHelper(mimeMessage, true, "UTF-8");
            helper.setFrom(senderEmail, "Arcana Team");
            helper.setTo(recipientEmail);
            helper.setSubject(subject);
            helper.setText(content, true); // HTML 텍스트 설정
        };

        mailSender.send(messagePreparator);
    }

    @Override
    @Transactional
    public void resetPassword(String email, String token, String newPassword) {
        String redisToken = redisService.getStringValue("password_reset:" + email);
        if (redisToken == null || !redisToken.equals(token)) {
            throw new CustomException("유효하지 않은 토큰입니다.", HttpStatus.BAD_REQUEST);
        }

        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        user.encodePassword(newPassword, passwordEncoder);
        userRepository.save(user);

        redisService.deleteValue("password_reset:" + email);
    }

    @Override
    public boolean isResetTokenValid(String email, String token) {
        String redisToken = redisService.getStringValue("password_reset:" + email);
        return redisToken != null && redisToken.equals(token);
    }

    @Override
    public User getUserByEmail(String email) {
        return userRepository.findByEmail(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));
    }

    private String generateRandomPassword() {
        String chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder password = new StringBuilder();
        Random random = new Random();

        for (int i = 0; i < 10; i++) { // 임시 비밀번호 길이: 10자리
            password.append(chars.charAt(random.nextInt(chars.length())));
        }
        return password.toString();
    }

    @Override
    public LoginResponseDto login(LoginDto loginDto) {
        try {
            // 사용자 인증
            Authentication authentication = authenticationManager.authenticate(
                new UsernamePasswordAuthenticationToken(loginDto.getEmail(), loginDto.getPassword())
            );

            // 인증이 성공하면 JWT 토큰 생성
            String accessToken = jwtUtil.generateAccessToken(authentication.getName());
            String refreshToken = jwtUtil.generateRefreshToken(authentication.getName());

            // Refresh Token을 Redis에 저장
            updateRefreshToken(authentication.getName(), refreshToken);

            // 사용자 정보 조회
            Long userId = getUserIdByEmail(authentication.getName());
            User user = getUserByEmail(authentication.getName());

            // 응답 DTO 생성 및 반환
            return new LoginResponseDto(
                accessToken,
                refreshToken,
                userId,
                user.getNickname(),
                user.getLanguage(),
                user.getMoney(),
                user.getHealth()
            );

        } catch (AuthenticationException e) {
            throw new CustomException("인증 실패: 이메일 또는 비밀번호가 올바르지 않습니다.", HttpStatus.UNAUTHORIZED);
        }
    }
}
