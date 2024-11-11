package com.arcane.arcana.user.service;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.dto.CustomUserDetails;
import com.arcane.arcana.user.repository.UserRepository;
import com.arcane.arcana.common.exception.CustomException;
import org.springframework.http.HttpStatus;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;

/**
 * 사용자 인증을 위한 서비스 인터페이스
 */
@Service
public class CustomUserDetailsService implements UserDetailsService {

    private final UserRepository userRepository;

    public CustomUserDetailsService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Override
    public UserDetails loadUserByUsername(String email) throws UsernameNotFoundException {
        User user = userRepository.findByEmailIncludingDeleted(email)
            .orElseThrow(() -> new CustomException("사용자를 찾을 수 없습니다.", HttpStatus.NOT_FOUND));

        if (user.isDeleted()) {
            throw new CustomException("탈퇴한 계정입니다.", HttpStatus.FORBIDDEN);
        }

        return new CustomUserDetails(user);
    }
}
