package com.arcane.arcana.user.service;

import com.arcane.arcana.common.entity.User;
import com.arcane.arcana.user.repository.UserRepository;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.stereotype.Service;

import java.util.Collections;

/**
 * 사용자 상세 정보 서비스를 제공
 */
@Service
public class CustomUserDetailsService implements UserDetailsService {

    private final UserRepository userRepository;

    public CustomUserDetailsService(UserRepository userRepository) {
        this.userRepository = userRepository;
    }

    @Override
    public UserDetails loadUserByUsername(String email) throws UsernameNotFoundException {
        User user = userRepository.findByEmail(email)
            .orElseThrow(() -> new UsernameNotFoundException("사용자를 찾을 수 없습니다."));

        return new org.springframework.security.core.userdetails.User(
            user.getEmail(),
            user.getPassword(),
            user.isEmailVerified(), // 계정 활성화 여부
            true, // 계정 만료 여부
            true, // 자격 증명 만료 여부
            !user.isDeleted(), // 계정 비활성화 여부
            Collections.emptyList()
        );
    }
}
